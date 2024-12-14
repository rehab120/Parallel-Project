
using FileDowanloaderPP.Repositry;
using Microsoft.AspNetCore.Mvc;
using FileDowanloaderPP.Models;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
namespace FileDowanloaderPP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownalodeController : ControllerBase
    {

        private readonly IHubContext<ProgressHub> hubContext;
        IFileRepositry file;

        public DownalodeController(IFileRepositry file, IHubContext<ProgressHub> hubContext)
        {
            this.file = file;
            this.hubContext = hubContext;
        }
        #region Sequential

        //[HttpPost("downloadMultiple")]
        //// method download sequential file It was meant to download files one by one and track the progress
        //public async Task<IActionResult> DownloadFilesWithProgressAsync([FromBody] List<FileDowalod> files)
        //{
        //    if (files == null || files.Count == 0)
        //    {
        //        return BadRequest("No files to download.");
        //    }

        //    string destinationFolder = Path.Combine(Path.GetTempPath(), "DownloadedFiles");
        //    var cancellationToken = HttpContext.RequestAborted;

        //    try
        //    {
        //        // Ensure destination folder exists
        //        if (!Directory.Exists(destinationFolder))
        //            Directory.CreateDirectory(destinationFolder);

        //        var totalWatch = Stopwatch.StartNew();

        //        // Sequentially download each file
        //        foreach (var file in files)
        //        {
        //            var watch = Stopwatch.StartNew();
        //            await DownloadAndSaveFileWithProgressAsync(file, destinationFolder, cancellationToken); // Sequential download
        //            watch.Stop();
        //            // Time Execution for each file
        //            Console.WriteLine(
        //                $"Execution time for downloading {file.FileName} :  {watch.ElapsedMilliseconds}ms");
        //        }

        //        totalWatch.Stop();
        //        // Total Execution time
        //        Console.WriteLine(
        //            $"The Execution time of the program is {totalWatch.ElapsedMilliseconds}ms");
        //        var TotalExecutionTime = totalWatch.ElapsedMilliseconds;

        //        // Return Success Response
        //        return Ok(new
        //        {
        //            Message = "Files downloaded successfully",
        //            DestinationFolder = destinationFolder,
        //            ExecutionTime = TotalExecutionTime
        //        });
        //    }
        //    catch (OperationCanceledException)
        //    {
        //        return StatusCode(499, "Request canceled."); // 499 is a common status code for client cancellation
        //    }
        //    catch (HttpRequestException)
        //    {
        //        return StatusCode(503, "Network error: Unable to connect to the server. Please check your internet connection.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error downloading files: {ex.Message}");
        //    }
        //}
        //private async Task DownloadAndSaveFileWithProgressAsync(FileDowalod fileD, string destinationFolder, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        using (HttpClient httpClient = new HttpClient())
        //        {
        //            httpClient.Timeout = TimeSpan.FromMinutes(10);

        //            var response = await httpClient.GetAsync(fileD.Url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        //            if (!response.IsSuccessStatusCode)
        //            {
        //                fileD.Statue = "Failed";
        //                Console.WriteLine($"Error downloading {fileD.FileName}: {response.StatusCode}");
        //                return;
        //            }

        //            string filePath = Path.Combine(destinationFolder, fileD.FileName);

        //            using (var httpStream = await response.Content.ReadAsStreamAsync(cancellationToken))
        //            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
        //            {
        //                long totalBytes = response.Content.Headers.ContentLength ?? -1;
        //                long totalRead = 0;
        //                byte[] buffer = new byte[8192];
        //                int bytesRead;

        //                // Read file content and write it to the file
        //                while ((bytesRead = await httpStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
        //                {
        //                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
        //                    totalRead += bytesRead;

        //                    // Calculate and update download progress
        //                    if (totalBytes > 0)
        //                    {
        //                        int progressPercentage = (int)((totalRead * 100) / totalBytes);
        //                        fileD.ProgressPercentageF = progressPercentage;

        //                        // Send progress update to the client via SignalR
        //                        await hubContext.Clients.All.SendAsync("ReceiveProgress", fileD.FileName, progressPercentage);
        //                        Console.WriteLine($"Downloading {fileD.FileName}: {progressPercentage}%");
        //                    }
        //                }
        //            }

        //            // Update file download status and save it to the repository
        //            fileD.DateDownaload = DateTime.Now;
        //            fileD.Statue = "Downloaded";

        //            // Add downloaded file details to repository
        //            file.Add(fileD);

        //        }
        //    }
        //    catch (OperationCanceledException)
        //    {
        //        fileD.Statue = "Canceled";
        //        Console.WriteLine($"Download canceled: {fileD.FileName}");
        //    }
        //    catch (HttpRequestException)
        //    {
        //        fileD.Statue = "Failed";
        //        Console.WriteLine($"Network error for {fileD.FileName}: Unable to connect to the server.");
        //        throw new HttpRequestException("Unable to connect to the server.");
        //    }
        //    catch (Exception ex)
        //    {
        //        fileD.Statue = "Failed";
        //        Console.WriteLine($"Error downloading {fileD.FileName}: {ex.Message}");
        //        throw;
        //    }
        //}

        #endregion



        #region Parralel
        //Method downloads multiple file in parallel
        [HttpPost("downloadMultiple")]
        public async Task<IActionResult> DownloadFilesWithProgressAsync([FromBody] List<FileDowalod> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("No files to download.");
            }

            string destinationFolder = Path.Combine(Path.GetTempPath(), "DownloadedFiles");
            var cancellationToken = HttpContext.RequestAborted;

            try
            {
                // Ensure destination folder exists
                if (!Directory.Exists(destinationFolder))
                    Directory.CreateDirectory(destinationFolder);

                var watch = Stopwatch.StartNew();

                List<Task<bool>> downloadTasks = new List<Task<bool>>();

                //start downloading all files in parallel
                foreach (var file in files)
                {
                    downloadTasks.Add(DownloadAndSaveFileWithProgressAsync(file, destinationFolder, cancellationToken));
                }
                // Wait for all dowaloads to complete 
                bool[] results = await Task.WhenAll(downloadTasks);
                watch.Stop();

                //EXecution time of whole program 
                Console.WriteLine(
                          $"The Execution time of the program is {watch.ElapsedMilliseconds}ms");
                var TotalExecutionTime = watch.ElapsedMilliseconds;

                // Return Sucess Response 
                if (results.All(success => success))
                {
                    return Ok(new
                    {
                        Message = "Files downloaded successfully",
                        DestinationFolder = destinationFolder,
                        ExecutionTime = TotalExecutionTime
                    });

                }
                else
                {
                    return StatusCode(500, "One or more files failed to download.");
                }
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Request canceled."); // 499 is a common status code for client cancellation
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error downloading files: {ex.Message}");
            }
        }

        private readonly object _lock = new object();
        //Helper Method to dowanload and save a single file with progress tracking in parallel
        private async Task<bool> DownloadAndSaveFileWithProgressAsync(FileDowalod fileD, string destinationFolder, CancellationToken cancellationToken)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromMinutes(10);

                    var response = await httpClient.GetAsync(fileD.Url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                    if (!response.IsSuccessStatusCode)
                    {
                        fileD.Statue = "Failed";
                        Console.WriteLine($"Error downloading {fileD.FileName}: {response.StatusCode}");
                        return false;
                    }

                    string filePath = Path.Combine(destinationFolder, fileD.FileName);

                    using (var httpStream = await response.Content.ReadAsStreamAsync(cancellationToken))
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                    {
                        long totalBytes = response.Content.Headers.ContentLength ?? -1;
                        long totalRead = 0;
                        byte[] buffer = new byte[8192];
                        int bytesRead;

                        // Read file content and write it to the file
                        while ((bytesRead = await httpStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                        {
                            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                            totalRead += bytesRead;
                            //Calculate and update download progress
                            if (totalBytes > 0)
                            {
                                int progressPercentage = (int)((totalRead * 100) / totalBytes);
                                fileD.ProgressPercentageF = progressPercentage;

                                //Send progress update to the client vis SingnalR
                                await hubContext.Clients.All.SendAsync("ReceiveProgress", fileD.FileName, progressPercentage);//gui
                                Console.WriteLine($"Downloading {fileD.FileName}: {progressPercentage}%");//console
                            }
                        }
                    }

                    // Update file download status and save it to the repository
                    fileD.DateDownaload = DateTime.Now;
                    fileD.Statue = "Downloaded";

                    lock (_lock)
                    {
                        // Add downloaded file details to repository
                        file.Add(fileD);
                    }
                    return true;
                }
            }
            catch (OperationCanceledException)
            {
                fileD.Statue = "Canceled";
                Console.WriteLine($"Download canceled: {fileD.FileName}");
                return false;
            }
            catch (Exception ex)
            {
                fileD.Statue = "Failed";
                Console.WriteLine($"Error downloading {fileD.FileName}: {ex.Message}");
                return false;
            }
        }


        #endregion



    }

}




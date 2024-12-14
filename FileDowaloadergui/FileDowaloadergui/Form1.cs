using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Sockets;
using System.Diagnostics;

namespace FileDowaloadergui
{
    public partial class Form1 : Form
    {
        private Dictionary<string, ProgressBar> fileProgressBars = new Dictionary<string, ProgressBar>();
        private List<FileDowalod> filesToDownload = new List<FileDowalod>();
        private HubConnection hubConnection;
        private CancellationTokenSource cancellationTokenSource;


        public Form1()
        {
            InitializeComponent();
            InitializeSignalR();

        }

        private async void InitializeSignalR()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7126/progresshub")
                .Build();

            hubConnection.On<string, int>("ReceiveProgress", (fileName, progress) =>
            {
                try
                {
                    this.Invoke(new Action(() =>
                    {
                        if (fileProgressBars.ContainsKey(fileName))
                        {
                            fileProgressBars[fileName].Value = progress;
                        }
                        var label = panelProgressBars.Controls
                        .OfType<Label>()
                        .FirstOrDefault(l => l.Text.StartsWith(fileName));

                        if (label != null)
                        {
                            label.Text = $"{fileName} - {progress}%"; // Update the file name and percentage
                        }
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating progress for {fileName}: {ex.Message}");
                }
            });

            hubConnection.Closed += async (error) =>
            {
                MessageBox.Show("Disconnected from progress updates. Attempting to reconnect...");
                await Task.Delay(2000);
                await hubConnection.StartAsync(); // Attempt to reconnect
            };

            try
            {
                await hubConnection.StartAsync(); // Start the connection
                MessageBox.Show("Connected to SignalR server.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to SignalR server: {ex.Message}");
            }
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (hubConnection != null)
            {
                try
                {
                    // Stop and dispose of the SignalR connection
                    hubConnection.StopAsync();
                    hubConnection.DisposeAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error closing SignalR connection: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            base.OnFormClosing(e); // Ensure the base class method is called
        }


        private void btnAddFile_Click(object sender, EventArgs e)
        {
            string url = txtUrl.Text;
            string fileName = Path.GetFileName(url);

            
            if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            {
                MessageBox.Show("Please enter a valid file URL.");
                return;
            }

            if (!string.IsNullOrEmpty(url))
            {
                if (fileProgressBars.ContainsKey(fileName))
                {
                    MessageBox.Show("This file is already added.");
                    return;
                }
                filesToDownload.Add(new FileDowalod { Url = url, FileName = fileName });
                lstFiles.Items.Add(fileName);

                // Add a progress bar for this file
                var progressBar = new ProgressBar
                {
                    Width = panelProgressBars.Width - 10,
                    Height = 30,
                    Style = ProgressBarStyle.Continuous,
                    Value = 0,
                    Tag = fileName
                };
                int spaceBetween = 5; //space between the label and the progress bar
                int yPosition = fileProgressBars.Count * (progressBar.Height + 30 + spaceBetween); // Adjust for space

                // Set the position of the progress bar and label
                progressBar.Location = new Point(10, yPosition);

                var progressLabel = new Label
                {

                    Location = new Point(10, yPosition - 5), // Place it above the progress bar
                    Size = new Size(panelProgressBars.Width - 10, 20),
                    Text = $"{fileName} - 0%", //  label text
                    TextAlign = ContentAlignment.MiddleLeft
                };
                panelProgressBars.Height = 500;

                panelProgressBars.Controls.Add(progressLabel);

                panelProgressBars.Controls.Add(progressBar);


                fileProgressBars[fileName] = progressBar;

                txtUrl.Clear();
                if (filesToDownload.Count > 0)
                {
                    panelProgressBars.Visible = true;
                }

                label1.Visible = true;
                label1.Text = "Execution Time : ";

            }
            else
            {
                MessageBox.Show("Please enter a valid file URL.");
            }
        }

        private async void btnDownaload_Click(object sender, EventArgs e)
        {
            if (filesToDownload.Count == 0)
            {
                MessageBox.Show("No files to download.");
                return;
            }



            string apiEndpoint = "https://localhost:7126/api/Downalode/downloadMultiple";
            ToggleDownloadButtons(true);
            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(10);

                    var jsonContent = new StringContent(
                        JsonSerializer.Serialize(filesToDownload),
                        Encoding.UTF8,
                        "application/json");



                    // Pass the CancellationToken to PostAsync
                    var response = await client.PostAsync(apiEndpoint, jsonContent, cancellationTokenSource.Token);

                    if (response.IsSuccessStatusCode)
                    {
                        // Parse the response to retrieve ExecutionTime
                        var responseContent = await response.Content.ReadAsStringAsync(cancellationTokenSource.Token);
                        var responseObject = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);

                        if (responseObject != null && responseObject.ContainsKey("executionTime"))
                        {
                            // Display the ExecutionTime from the backend
                            var executionTime = responseObject["executionTime"]?.ToString();
                            label1.Text = $"Execution Time: {executionTime} ms";
                        }
                        else
                        {
                            MessageBox.Show("ExecutionTime is missing in the response.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        MessageBox.Show("Files downloaded successfully!");
                    }
                    else
                    {
                        MessageBox.Show($"Error: {response.StatusCode}\nDetails: {await response.Content.ReadAsStringAsync()}",
                                        "Error",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                }
            }

            catch (OperationCanceledException)
            {
                MessageBox.Show("Download canceled.", "Canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                panelProgressBars.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

                cancellationTokenSource = null;
                ToggleDownloadButtons(false);
            }
        }






        private void lstFiles_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnCancelDownload_Click(object sender, EventArgs e)
        {
            cancellationTokenSource?.Cancel();


        }
        private void ToggleDownloadButtons(bool isDownloading)
        {
            btnDownaload.Enabled = !isDownloading;
            btnCancelDownload.Enabled = isDownloading;
            btnAddFile.Enabled = !isDownloading;
            btnReset.Enabled = !isDownloading;
            

        }
        private void btnReset_Click(object sender, EventArgs e)
        {


            // Clear the list of files and reset the progress bars
            filesToDownload.Clear();  // Clear the list of files
            fileProgressBars.Clear();  // Clear the dictionary of progress bars
            lstFiles.Items.Clear();    // Clear the ListBox (lstFiles)

            // Clear the previous progress bars and labels
            panelProgressBars.Controls.Clear();

            // Hide the progress panel
            panelProgressBars.Visible = false;
            label1.Visible = false;

        }

        private void panelProgressBars_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

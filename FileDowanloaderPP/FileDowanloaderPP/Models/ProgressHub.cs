using Microsoft.AspNetCore.SignalR;

namespace FileDowanloaderPP.Models
{
    public class ProgressHub : Hub
    {
        public async Task UpdateProgress(string fileName, int progress)
        {
            await Clients.All.SendAsync("ReceiveProgress", fileName, progress);
        }
    }
}

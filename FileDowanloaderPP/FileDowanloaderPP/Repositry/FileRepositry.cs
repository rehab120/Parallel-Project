using FileDowanloaderPP.Models;


namespace FileDowanloaderPP.Repositry
{
    public class FileRepositry:IFileRepositry
    {
        Context context;
        public FileRepositry(Context context)
        {
            this.context = context;
        }

        public void Add(Models.FileDowalod file)
        {
            context.Files.Add(file);
            context.SaveChanges();
        }

    }
}

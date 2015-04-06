namespace BaseTools.Core.Network
{
    using BaseTools.Core.DataAccess;
    using BaseTools.Core.FileSystem;
    using BaseTools.Core.Ioc;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public class FileLoader
    {
        private static readonly FileLoader instance = new FileLoader();

        //private static readonly IFileSystemProvider fileSystemProvider = Factory.Common.GetInstance<IFileSystemProvider>();

        public static FileLoader Instance
        {
            get
            {
                return instance;
            }
        }

        public async Task<DataResponse<bool>> SaveFile(string webFilePath, string localPath)
        {
            DataResponse<bool> result = new DataResponse<bool>();
            var response = await HttpRequestSender.Send(webFilePath).ConfigureAwait(false);
            if (response.IsSuccess)
            {
                using (response.Result)
                {
                    var fileSystemProvider = Factory.Common.GetInstance<IFileSystemProvider>();
                    using (var localStream = await fileSystemProvider.OpenFileAsync(localPath, FileOpeningMode.Create))
                    {
                        response.Result.ResponseStream.CopyTo(localStream);
                    }

                    result.Result = true;
                }
            }
            else
            {
                result.Error = response.Error;
            }

            return result;
        }
    }
}

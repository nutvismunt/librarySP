using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;

namespace BusinessLayer.Parser
{
    public class UrlPicDownload
    {
        //загрузка изображения книги по ссылке
        public void SaveImage(string filename, string imageUrl)
        {
            //не найдя аналог OpenRead подходящий для метода использую WebCleint
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(imageUrl);
            Bitmap bitmap; bitmap = new Bitmap(stream);

            if (bitmap != null)
            {
                bitmap.Save(filename);
            }

            stream.Flush();
            stream.Close();
            client.Dispose();
        }
    }
}

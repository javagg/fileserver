using System.Net;
using System.Text;

var url = "http://localhost:5000/";
var file = @"e:\images.zip";
var client = new WebClient();
client.QueryString["directory"] = "uploads";
var res = client.UploadFile(url, "POST", file);
Console.WriteLine($"res: {Encoding.UTF8.GetString(res)}");


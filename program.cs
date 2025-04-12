using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using PdfSharp.Pdf;
using System.Threading;

namespace PruebaLoginOrbi
{
    class Program
    {
        static void Main(string[] args)
        {
            string usuario = "Carrascoyenni6@gmail.com";
            string clave = "40218870141";

            string resultado = "";
            string mensaje = "";

            
            string carpeta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ReportesOrbi");
            Directory.CreateDirectory(carpeta);

            string screenshotPath = Path.Combine(carpeta, "captura_orbi.png");
            string htmlPath = Path.Combine(carpeta, "reporte_orbi.html");
            string pdfPath = Path.Combine(carpeta, "reporte_orbi.pdf");

            IWebDriver driver = new ChromeDriver();

            try
            {
                driver.Navigate().GoToUrl("https://orbi.edu.do/orbi/");
                driver.Manage().Window.Maximize();
                Thread.Sleep(3000);

                driver.FindElement(By.Id("username")).SendKeys(usuario);
                driver.FindElement(By.Id("password")).SendKeys(clave);
                driver.FindElement(By.CssSelector("button[type='submit']")).Click();

                Thread.Sleep(4000);
                bool loginCorrecto = !driver.Url.Contains("login");

                if (loginCorrecto)
                {
                    resultado = " LOGIN EXITOSO";
                    mensaje = "El usuario pudo ingresar correctamente a la plataforma ORBI.";
                }
                else
                {
                    resultado = " LOGIN FALLIDO";
                    mensaje = "Credenciales incorrectas o acceso denegado.";
                }

                
                Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
                ss.SaveAsFile(screenshotPath);
                string html = GenerarHTML(resultado, mensaje, "captura_orbi.png"); 
                File.WriteAllText(htmlPath, html);
                PdfDocument pdf = PdfGenerator.GeneratePdf(html, PdfSharp.PageSize.A4);
                pdf.Save(pdfPath);

                Console.WriteLine(" PDF generado en: " + pdfPath);
                Console.WriteLine("Presiona cualquier tecla para cerrar...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(" Error durante la prueba: " + ex.Message);
            }
            finally
            {
                Console.ReadKey();
                driver.Quit();
            }
        }


        //generar el HTML
        static string GenerarHTML(string resultado, string mensaje, string imagen)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Reporte de Prueba - ORBI</title>
    <style>
        body {{ font-family: Arial; padding: 30px; background: #f5f5f5; }}
        h1 {{ color: #333; }}
        .resumen {{ background: #fff; padding: 20px; border-radius: 8px; box-shadow: 0 0 10px #ccc; }}
        img {{ max-width: 100%; margin-top: 20px; border: 1px solid #ddd; }}
    </style>
</head>
<body>
    <div class='resumen'>
        <h1>{resultado}</h1>
        <p>{mensaje}</p>
        <h3>Captura de pantalla:</h3>
        <img src='{imagen}' />
    </div>
</body>
</html>";
        }
    }
}


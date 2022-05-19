using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ASCIIart
{
    class Program
    {
        static void Main(string[] args)
        {
            //list of characters ordered by 'pixel density'
            string pixelChars = "`^\",:;Il!i~+_-?][}{1)(|\\/tfjrxnuvczXYUJCLQOZmwqpdbkhao*#0MW&8%B@$";
            //permanently use BightnessMode.Lightness since it looks best
            BrightnessMode mode = BrightnessMode.Lightness;


            //get image bitmap from file path
            string imagePath = "";
            if (args.Length != 0)
            {
                imagePath = args[0];
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Drag an image file onto this .exe file.");
                Console.WriteLine("For better results right click the window title bar, select Properties and make the font size smaller.");
                while (Console.ReadLine() == null) { }
                return;
            }

            //create a bitmap from the image
            Bitmap unscaledInputImage;            
            try
            {
                unscaledInputImage = new Bitmap(imagePath);                
            }
            catch (Exception err)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(imagePath);
                Console.WriteLine(err.Message);
                Console.WriteLine(err.StackTrace);
                while (Console.ReadLine() == null) { }
                return;
            }

            Console.Title = imagePath;

            //resize image if too large, keeping aspect ratio
            //use 0.99 * console max dimensions to ensure no text wrapping
            int maxWidth = (int)Math.Floor(0.99 * Console.LargestWindowWidth);
            int maxHeight = (int)Math.Floor(0.99* Console.LargestWindowHeight);
            double scaleRate = 1;

            if ((double)unscaledInputImage.Width*scaleRate*2 > maxWidth)
            {
                scaleRate *= (double) maxWidth / (double) (unscaledInputImage.Width*scaleRate*2);
            }
            if ((double)unscaledInputImage.Height*scaleRate > maxHeight)
            {
                scaleRate *= (double) maxHeight / (double) (unscaledInputImage.Height*scaleRate);
            }
            Bitmap inputImage = new Bitmap(unscaledInputImage, 
                (int)Math.Floor((double)unscaledInputImage.Width * scaleRate*2), 
                (int)Math.Floor((double)unscaledInputImage.Height * scaleRate));

            unscaledInputImage.Dispose();            
            Console.SetWindowSize((int)Math.Ceiling(1.01* inputImage.Width), (int)Math.Ceiling(1.01 * inputImage.Height));

            //image data matrices
            Color[,] imagePixels = new Color[inputImage.Width, inputImage.Height];
            int[,] imageBrightness = new int[inputImage.Width, inputImage.Height];
            string[,] outputImage = new string[inputImage.Width, inputImage.Height];

            //get the brightness and color data for each pixel of the bitmap and print that character to the console
            for (int j = 0; j < inputImage.Height; j++)
            {
                for (int i = 0; i < inputImage.Width; i++)
                {
                    imagePixels[i, j] = inputImage.GetPixel(i, j);
                    imageBrightness[i, j] = Brightness(imagePixels[i, j], mode);
                    int charIndex = (int)Math.Round(((double)imageBrightness[i, j] / 256) * (pixelChars.Length - 1));
                    outputImage[i, j] = pixelChars[charIndex].ToString();
                    Console.ForegroundColor = GetConsoleColor(imagePixels[i, j]);
                    Console.Write(outputImage[i, j]);        
                }
                Console.Write("\n");
            }
            //reset console colour to white, to avoid it being left on black and making all text invisible
            Console.ForegroundColor = ConsoleColor.White;
            while (Console.ReadLine() == null){}     
        }

        //determine the brightness of the colour passed
        enum BrightnessMode {Average, Lightness, Luminosity};
        static int Brightness(Color color,BrightnessMode mode)
        {
            int brightness = 0;
            //multiple options for calculating brightness
            switch (mode)
            {
                case BrightnessMode.Average:
                    brightness = (color.R + color.G + color.B) / 3;
                    break;
                case BrightnessMode.Lightness:
                    brightness = (int)(Math.Max(color.R, Math.Max(color.G,color.B)) + Math.Min(color.R, Math.Min(color.G, color.B)) )/ 2;
                    break;
                case BrightnessMode.Luminosity:
                    brightness = (int) (0.21*color.R + 0.72*color.G + 0.07*color.B);
                    break;
                default:
                    break;
            }            
            return brightness;
        }

        //determine which 16 console colours best matches the colour passed
        static ConsoleColor GetConsoleColor(Color color)
        {
            int[,] consoleRGB = { {0,0,0},{0,0,139},{0,100,0},{0,139,139},{139,0,0},{139,0,139},{139,139,0},
                {128,128,128},{169,169,169},{0,0,255},{0,128,0},{0,255,255},{255,0,0},{255,0,255},{255,255,0},{255,255,255} };
            //Black        #000000
            //DarkBlue     #00008B
            //DarkGreen    #006400
            //DarkCyan     #008B8B
            //DarkRed      #8B0000
            //DarkMagenta  #8B008B
            //DarkYellow   #8B8B00   
            //Gray         #808080
            //DarkGray     #A9A9A9
            //Blue         #0000FF
            //Green        #008000
            //Cyan         #00FFFF
            //Red          #FF0000
            //Magenta      #FF00FF
            //Yellow       #FFFF00
            //White        #FFFFFF

            double bestColorDist = double.MaxValue;
            int bestColor = 0;
            double colorDist = 0;

            //console colour that has shortest RGB vector distance to the colour passed is the best match
            for (int i = 0; i < consoleRGB.GetLength(0);i++)
            {
                double dR = (Math.Abs(consoleRGB[i, 0] - color.R));
                double dG = (Math.Abs(consoleRGB[i, 1] - color.G));
                double dB = (Math.Abs(consoleRGB[i, 2] - color.B));

                colorDist = Math.Sqrt(dR * dR + dG * dG + dB * dB);                
                if (colorDist < bestColorDist)
                {
                    bestColorDist = colorDist;
                    bestColor = i;
                }
            }
            return (ConsoleColor) bestColor;
        }
    }
}

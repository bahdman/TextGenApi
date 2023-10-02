using Microsoft.AspNetCore.Mvc;
using Whisper.net;
using NAudio.Wave;
using Whisper.net.Ggml;
using Test.Controllers;

namespace Test.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IWebHostEnvironment _env;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IActionResult> Get()
    {
        var videoFile = "C:\\Users\\user\\source\\repos\\HNG\\Test\\wwwroot\\VideoFiles\\recording.mp4";
        var wavFileName = "C:\\Users\\user\\source\\repos\\HNG\\Test\\audio.wav";

        if(System.IO.Path.Exists(wavFileName))
        {
            // var status = GenerateAudioFile(videoFile);
            // var wavWrite = await Sample();
            // var audio = DownloadModel(wavFileName, GgmlType.Base);

            // var items = Test().Result;
            var handler = new Testt();
            var response = handler.Transcribe(wavFileName);
            return Ok(response);
        }

        

        return BadRequest("OMo broo");
    }

    [NonAction]
    public async Task<IList<string>> Test()
    {

        var wavFileName = "C:\\Users\\user\\source\\repos\\HNG\\Test\\wwwroot\\AudioFiles\\audio.wav";
        using var fileStream = System.IO.File.OpenRead(wavFileName);
        var item = new List<string>();

        var modelFileName = "./Whisper/ggml-base.bin";
        var ggmlType = GgmlType.Base;


        if (!System.IO.File.Exists(modelFileName))
            {
                await DownloadModel(modelFileName, ggmlType);
            }
        

        using var whisperFactory = WhisperFactory.FromPath("ggml-base.bin");

            // This section creates the processor object which is used to process the audio file, it uses language `auto` to detect the language of the audio file.
            using var processor = whisperFactory.CreateBuilder()
                .WithLanguage("auto")
                .Build();
        await foreach (var result in processor.ProcessAsync(fileStream))
            {
                item.Add(result.Text);

                Console.WriteLine($"{result.Start}->{result.End}: {result.Text}");
            }

            return item;

    }

        [NonAction]
        public async Task<bool>  Sample()
        {
            var ggmlType = GgmlType.Base;
            var modelFileName = "./Whisper/ggml-base.bin";
            var wavFileName = "C:\\Users\\user\\source\\repos\\HNG\\Test\\output_audio.wav";
            

            // This section detects whether the "ggml-base.bin" file exists in our project disk. If it doesn't, it downloads it from the internet
            if (!System.IO.File.Exists(modelFileName))
            {
                await DownloadModel(modelFileName, ggmlType);
            }

            // This section creates the whisperFactory object which is used to create the processor object.
            using var whisperFactory = WhisperFactory.FromPath(modelFileName);

            // This section creates the processor object which is used to process the audio file, it uses language `auto` to detect the language of the audio file.
            using var processor = whisperFactory.CreateBuilder()
                .WithLanguage("auto")
                .Build();

            using var fileStream = System.IO.File.OpenRead(wavFileName);
            var item = new List<string>();

            // This section processes the audio file and prints the results (start time, end time and text) to the console.
            await foreach (var result in processor.ProcessAsync(fileStream))
            {
                item.Add(result.Text);
                Console.WriteLine($"{result.Start}->{result.End}: {result.Text}");
            }

            return true;
        }
    
        [NonAction]
        private static async Task DownloadModel(string fileName, GgmlType ggmlType)
        {
            Console.WriteLine($"Downloading Model {fileName}");
            using var modelStream = await WhisperGgmlDownloader.GetGgmlModelAsync(ggmlType);
            using var fileWriter = System.IO.File.OpenWrite(fileName);
            await modelStream.CopyToAsync(fileWriter);
        }



        [NonAction]
        private bool GenerateAudioFile(string filePath)
        {
            // Specify the path to your video file
            string videoFilePath = "path_to_video.mp4";

            // Create a MediaFoundationReader to read audio from the video file
            using (var reader = new MediaFoundationReader(filePath))
            {
                // Specify the output audio file format (e.g., WAV)
                var outputFormat = new WaveFormat(44100, 16, 2); // Sample rate, bit depth, channels

                // Create a WaveFileWriter to write the extracted audio to a file
            using (var writer = new WaveFileWriter("output_audio.wav", outputFormat))
            {
                // Read and write audio data in chunks
                byte[] buffer = new byte[8192];
                int bytesRead;
                while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    writer.Write(buffer, 0, bytesRead);
                }
            }


                return true;
            }
        }

        [NonAction]
        public bool ExtractAudioFromVideo(string videoFilePath, string audioOutputPath)
        {
            try
            {
                using (var reader = new MediaFoundationReader(videoFilePath))
                {
                    var pcmStream = WaveFormatConversionStream.CreatePcmStream(reader); // 16 kHz, 16-bit, mono
                    WaveFileWriter.CreateWaveFile(audioOutputPath, pcmStream);
                }

                return true;

                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);  
                return false;          
            }
        }

}


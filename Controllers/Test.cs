using Whisper.net.Ggml;
using Whisper.net;
using Test.Models;
// using System.Speech.Recognition;
using Speechmatics.Transcribe;
using Speechmatics.Transcribe.Model;

namespace Test.Controllers
{
    public class Testt
    {
        public async Task<List<TranscribeData>> Transcribe(string path)
        {
            // We declare three variables which we will use later, ggmlType, modelFileName and wavFileName
            var ggmlType = GgmlType.Base;
            var modelFileName = "ggml-base.bin";

            // This section detects whether the "ggml-base.bin" file exists in our project disk. If it doesn't, it downloads it from the internet
            if (!File.Exists(modelFileName))
            {
                await DownloadModel(modelFileName, ggmlType);
            }

            // This section creates the whisperFactory object which is used to create the processor object.
            using var whisperFactory = WhisperFactory.FromPath("ggml-base.bin");

            // This section creates the processor object which is used to process the audio file, it uses language `auto` to detect the language of the audio file.
            using var processor = whisperFactory.CreateBuilder()
                .WithLanguage("auto")
                .Build();

            var audioPath = "C:\\Users\\user\\source\\repos\\HNG\\Test\\wwwroot\\AudioFiles\\audio.wav";
            using var fileStream = File.OpenRead(path);
            var list = new List<TranscribeData>();
            // This section processes the audio file and prints the results (start time, end time and text) to the console.
            await foreach (var result in processor.ProcessAsync(fileStream))
            {
                list.Add(new TranscribeData()
                {
                    Start = result.Start,
                    End = result.End,
                    Text = result.Text,
                });
            }
            if (fileStream != null) fileStream.Close();
            //Delete temporary wav audio
            File.Delete(audioPath);
            return list;
        }
        private static async Task DownloadModel(string fileName, GgmlType ggmlType)
        {
            Console.WriteLine($"Downloading Model {fileName}");
            using var modelStream = await WhisperGgmlDownloader.GetGgmlModelAsync(ggmlType);
            using var fileWriter = File.OpenWrite(fileName);
            await modelStream.CopyToAsync(fileWriter);
        }

        // private string TestM(string path)
        // {
        //     var instance = new LanguageIdentificationConfig();
        //     var test = instance.ExpectedLanguages;
        // }
    }
}
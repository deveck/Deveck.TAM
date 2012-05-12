using System;
using System.IO;
using Deveck.TAM.Core;
using NAudio.Wave;

namespace Deveck.TAM.Actions
{
	public class PlaywavAction : IAction
	{
		private String _file;
		
		public PlaywavAction(String file)
		{
			_file = file;
						
			using (var reader = CreateWavStream(_file))
			{
				var newFormat = new WaveFormat(48000, 16, 1);
				using (var conversionStream = new WaveFormatConversionStream(newFormat, reader))
				{
					WaveFileWriter.CreateWaveFile(_file + ".sampled", conversionStream);
				}
			}
		}
		
		public void Execute(ICall call)
		{
			call.PlayAudioFile(_file + ".sampled");
		}
		
		private WaveStream CreateWavStream(String file)
		{
			FileInfo f = new FileInfo(file);
			if(f.Extension.Equals(".wav", StringComparison.InvariantCultureIgnoreCase))
				return new WaveFileReader(file);
			else
				return new Mp3FileReader(file);
		}
	}
}

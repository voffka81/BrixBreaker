using System;
using System.IO;

namespace WPF.Sound
{
    // Wav file structure
    // Header chunk

    //Type   Byte Offset  Description
    // Dword       0       Always ASCII "RIFF"
    // Dword       4       Number of bytes in the file after this value (= File Size - 8)
    // Dword       8       Always ASCII "WAVE"

    // Format Chunk

    // Type   Byte Offset  Description
    // Dword       12      Always ASCII "fmt "
    // Dword       16      Number of bytes in this chunk after this value
    // Word        20      Data format PCM = 1 (i.e. Linear quantization)
    // Word        22      Channels Mono = 1, Stereo = 2
    // Dword       24      Sample Rate per second e.g. 8000, 44100
    // Dword       28      Byte Rate per second (= Sample Rate * Channels * (Bits Per Sample / 8))
    // Word        32      Block Align (= Channels * (Bits Per Sample / 8))
    // Word        34      Bits Per Sample e.g. 8, 16

    // Data Chunk

    // Type   Byte Offset  Description
    // Dword       36      Always ASCII "data"
    // Dword       40      The number of bytes of sound data (Samples * Channels * (Bits Per Sample / 8))
    // Buffer      44      The sound data

    class MediaFile
    {

        byte[] HeaderData = new byte[44];
        byte[] FileData;
        string actualName;

        public string ActualName
        {
            get
            {
                return actualName;
            }
        }

        /// <summary>
        /// Return length of wav file in seconds
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                //time = ((total size - header size) / (sample rate * (bit rate / 8))) / number of channels
                byte[] val = BlockCopy(this.HeaderData, 24, 4);
                double SamlpeRate = BitConverter.ToUInt32(val, 0);
                byte BitRate = HeaderData[34];
                byte Channels = HeaderData[22];

                double time = ((FileData.Length - HeaderData.Length) / (SamlpeRate * (BitRate / 8))) / Channels;

                return TimeSpan.FromSeconds(time);
            }
        }

        public byte[] AudioData
        {
            get
            {
                return this.FileData;
            }
        }


        public MediaFile(string AudioFileName)
        {
            // open filestream to read
            FileStream fs = new FileStream(AudioFileName, FileMode.Open);
            // read all bytes to array
            FileData = new byte[fs.Length];
            fs.Read(FileData, 0, (int)fs.Length);
            // close file stream

            actualName = Path.GetFileName(fs.Name);
            int fileExtPos = actualName.LastIndexOf(".");
            if (fileExtPos >= 0)
                actualName = actualName.Substring(0, fileExtPos);
            fs.Close();

            Array.Copy(FileData, HeaderData, HeaderData.Length);
        }

        private byte[] BlockCopy(byte[] Source, long Offset, long Bytes)
        {
            byte[] Destination = new byte[Bytes];

            try
            {
                Array.Copy(Source, Offset, Destination, 0, Bytes);
            }
            catch
            {
                throw new Exception();
            }

            return Destination;
        }
    }
}

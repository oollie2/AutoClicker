namespace AutoClicker.Classes
{
    internal class Button
    {
        public uint UpCode { get; set; }
        public uint DownCode { get; set; }
        public Button(uint _downCode, uint _upCode)
        {
            DownCode = _downCode;
            UpCode = _upCode;
        }
    }
}

using Laba6;

namespace laba6
{

    public class ERROR : Exception
    {
        public ERROR(string message) : base(message) { }

    }

    public class ContinentNotFoundException : ERROR
    {
        public ContinentNotFoundException(string message) : base(message) { }
    }

    public class NoSeasException : ERROR
    {
        public NoSeasException(string message) : base(message) { }
    }

    public class NoIslandsException : ERROR
    {
        public NoIslandsException(string message) : base(message) { }
    }
    public class InvalidCoordinatesException : ERROR
    {
        public InvalidCoordinatesException(string message) : base(message) { }
    }
    public class InvalidFormatCoordinatesException : ERROR
    {
        public InvalidFormatCoordinatesException(string message) : base(message) { }
    }
}

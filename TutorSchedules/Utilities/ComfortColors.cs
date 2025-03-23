using System.Drawing;

namespace TutorSchedules.Utilities;

public static class ComfortColors
{
    public static string GetColor(int? level)
    {
        if (level is null)
            return "rgb(0, 0, 0)";
        int value = level.Value;
        int red = Interpolate(value, 10, 5, 0, 255);
        int green = Interpolate(value, 0, 5, 0, 255);
        return $"rgb({red}, {green}, 0)";
    }

    private static int Interpolate(int x, int minIn, int maxIn, int minOut, int maxOut)
    {
        double m = ((double)maxOut - minOut)/(maxIn - minIn);
        double b = -m * minIn + minOut;
        return (int)Math.Clamp(m * x + b, minOut, maxOut);
    }

    public static string GetStyleString(int? level)
    {
        return $"style=\"background: linear-gradient(to right, white, {GetColor(level)}\"";
    }
}
﻿class Duplicate
{
    public readonly string From;
    public readonly string To;

    public Duplicate(string from, string? to)
    {
        From = from;
        if (to is null)
        {
            To = from;
        }
        else
        {
            To = to;
        }
    }
}
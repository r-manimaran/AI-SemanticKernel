using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesApp;

public class Movie
{
    public string Title { get; set; }
    public string Director { get; set; }
    public int ReleaseYear { get; set; }
    public double Rating { get; set; }
    public bool IsAvailableOnStreaming { get; set; }
    public List<string> Tags { get; set; }
    public string MusicComposer { get; set; }
}

public class MovieResult
{
    public List<Movie> Movies { get; set; } = new();
}
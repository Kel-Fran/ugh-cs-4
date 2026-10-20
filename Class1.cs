namespace ugh_cs_4;

public interface IContent
{
    string Title {get;}
    int Length {get;}
}

public sealed record class Video(string Title, int Length) : IContent;

public sealed record class Movie(Video Video) : IContent
{
    public string Title => Video.Title;
    public int Length => Video.Length;
}

public sealed record class Series(string Title, IReadOnlyCollection<Video> Episodes) : IContent
{
    public int Length => Episodes.Sum(x => x.Length);

    public Series(string title, Video firstEpisode): this(title, [firstEpisode]) {}
}

public sealed record class StreamingPlatform(List<IContent> Contents) : IEnumerable<IContent>, IEnumerable
{
    public void AddContent(IContent content) => Contents.Add(content);
    public IContent? Longest => Contents.MaxBy(x => x.Length);

    IEnumerator IEnumerable.GetEnumerator() => List.GetEnumerator();
    IEnumerator<IContent> GetEnumerator() => List.GetEnumerator();

    public static StreamingPlatform FromUTF8File(FileInfo file)
    {
        using var f =file.OpenText();
        return Parse(f.ReadToEnd());
    }

    public static StreamingPlatform Parse(ReadOnlySpan<char>src)
    {
        var contents = new List<IContent>();
        foreach (var range in src.Split('\n'))
        {
            var line = src[range].Trim();
            // [M]title: length <- movie
            // [S]title\0 title: length[\0 title: length]* <- series

            if (line.StartsWith('M'))
            {
                var p = line.LastIndexOf(':');
                contents.Add(new Movie(new Video(line[1..p].ToString(), int.Parse(line[(p + 1)..]))));
            }
            else if (line.StartsWith('S'))
            {
                var p = line.IndexOf('\0');
                var title = line[1..p];
                var episodesSrc = line[(p+1)..];
                var episodes = new List<Video>(episodesSrc.Count(',')+1);
                foreach (var erange in episodesSrc.Split('\0'))
                {
                    p = line.LastIndexOf(':');
                    episodes.Add(new Video(line[1..p].ToString(), int.Parse(line[(p + 1)..])));
                }
                contents.Add(new Series(title.ToString(), episodes));
            }
        }

        return new(contents);
    }
}
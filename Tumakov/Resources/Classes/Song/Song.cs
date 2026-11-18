namespace Tumakov
{
    class Song
    {
        public string name { get; set; }
        public string author { get; set; }
        Song prev;

        public Song()
        {
            name = "Unknown";
            author = "Unknown";
            prev = null;
        }

        public Song(string name, string author)
        {
            this.name = name;
            this.author = author;
            prev = null;
        }

        public Song(string name, string author, Song prev)
        {
            this.name = name;
            this.author = author;
            this.prev = prev;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public void SetAuthor(string author)
        {
            this.author = author;
        }

        public void SetPrev(Song prev)
        {
            this.prev = prev;
        }

        public void PrintInfo()
        {
            Console.WriteLine(Title());
        }

        public string Title()
        {
            return $"{name} — {author}";
        }

        public override bool Equals(object d)
        {
            if (d is Song otherSong)
            {
                return this.name == otherSong.name && this.author == otherSong.author;
            }
            return false;
        }
    }
}

package music.model;

public class Track {
    private final int id;
    private final String title;
    private final String artist;

    public Track(int id, String title, String artist) {
        this.id = id;
        this.title = title;
        this.artist = artist;
    }

    public String getArtist() {
        return artist;
    }
    public String getTitle() {
        return title;
    }
    public int getId() {
        return id;
    }

    @Override
    public String toString() {
        return "Track{" +
                "id=" + id +
                ", title='" + title + '\'' +
                ", artist='" + artist + '\'' +
                '}';
    }
}

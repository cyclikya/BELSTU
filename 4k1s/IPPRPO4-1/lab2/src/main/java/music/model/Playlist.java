package music.model;

import java.util.ArrayList;
import java.util.List;

public class Playlist {
    private final int id;
    private final String name;
    private final User owner;
    private final List<Track> tracks = new ArrayList<>();

    public Playlist(int id, String name, User owner) {
        this.id = id;
        this.name = name;
        this.owner = owner;
    }

    public int getId() {
        return id;
    }
    public String getName() {
        return name;
    }
    public User getOwner() {
        return owner;
    }

    public void addTrack(Track track){
        tracks.add(track);
    }

    public List<Track> getTracks(){
        return List.copyOf(tracks);
    }

    public boolean hasTrack(int trackId) {
        for (Track t : tracks) {
            if (t.getId() == trackId) {
                return true;
            }
        }
        return  false;
    }

    @Override
    public String toString() {
        return "Playlist{" +
                "id=" + id +
                ", name='" + name + '\'' +
                ", owner=" + owner +
                ", tracks=" + tracks +
                '}';
    }
}

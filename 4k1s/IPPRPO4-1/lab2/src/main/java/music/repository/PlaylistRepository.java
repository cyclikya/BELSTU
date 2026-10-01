package music.repository;

import music.model.Playlist;

public interface PlaylistRepository {
    void save(Playlist playlist);
    Playlist findById(int id);
}

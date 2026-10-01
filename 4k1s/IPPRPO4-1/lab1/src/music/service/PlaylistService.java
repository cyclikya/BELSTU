package music.service;

import music.model.Playlist;
import music.model.Track;
import music.model.User;
import music.repository.PlaylistRepository;

public class PlaylistService {
    private final PlaylistRepository repository;

    public PlaylistService(PlaylistRepository repository) {
        this.repository = repository;
    }

    public Playlist createPlaylist(int id, String name, User owner) {
        Playlist playlist = new Playlist(id, name, owner);
        repository.save(playlist);
        return playlist;
    }

    public void addTrackToPlaylist(int playlistId, Track track){
        Playlist playlist = repository.findById(playlistId);
        if(playlist == null){
            throw new IllegalArgumentException("Плейлист не найден");
        }
        if(playlist.hasTrack(track.getId())) {
            throw new IllegalStateException("Трек уже есть в плейлисте");
        }
        playlist.addTrack(track);
        repository.save(playlist);
    }
}
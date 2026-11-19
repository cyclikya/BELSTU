package music.service;

import music.model.Playlist;
import music.model.Track;
import music.model.User;
import music.repository.PlaylistRepository;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class PlaylistService {
    private static final Logger LOGGER = LoggerFactory.getLogger(PlaylistService.class);

    private final PlaylistRepository repository;

    public PlaylistService(PlaylistRepository repository) {
        this.repository = repository;
    }

    public Playlist createPlaylist(int id, String name, User owner) {
        Playlist playlist = new Playlist(id, name, owner);
        repository.save(playlist);
        LOGGER.info("Создан плейлист: id={}, name={}, owner={}", id, name, owner.getName());
        return playlist;
    }

    public void addTrackToPlaylist(int playlistId, Track track) {
        Playlist playlist = repository.findById(playlistId);
        if (playlist == null) {
            LOGGER.error("Невозможно добавить трек {}: плейлист {} не найден", track.getId(), playlistId);
            throw new IllegalArgumentException("Плейлист не найден");
        }
        if (playlist.hasTrack(track.getId())) {
            LOGGER.warn("Трек {} уже присутствует в плейлисте {}", track.getId(), playlistId);
            throw new IllegalStateException("Трек уже есть в плейлисте");
        }
        playlist.addTrack(track);
        repository.save(playlist);
        LOGGER.info("Трек {} успешно добавлен в плейлист {}", track.getId(), playlistId);
    }
}

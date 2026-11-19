package music;

import music.model.Playlist;
import music.model.Track;
import music.model.User;
import music.repository.InMemoryPlaylistRepository;
import music.repository.PlaylistRepository;
import music.service.PlaylistService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class Main {
    private static final Logger LOGGER = LoggerFactory.getLogger(Main.class);

    public static void main(String[] args) {
        PlaylistRepository repository = new InMemoryPlaylistRepository();
        PlaylistService service = new PlaylistService(repository);

        User user = new User(1, "Виолетта");
        Playlist playlist = service.createPlaylist(1, "хайп", user);

        Track track1 = new Track(1, "Днями и ночами", "Бушидо Джо");
        Track track2 = new Track(2, "Колыбельная", "Мильковский");

        service.addTrackToPlaylist(playlist.getId(), track1);
        service.addTrackToPlaylist(playlist.getId(), track2);

        try {
            service.addTrackToPlaylist(1, track1);
        } catch (IllegalStateException e) {
            LOGGER.debug("Ожидаемое исключение при добавлении того же объекта трека", e);
        }

        Track track1Copy = new Track(1, "Днями и ночами", "Бушидо Джо");
        try {
            service.addTrackToPlaylist(1, track1Copy);
        } catch (IllegalStateException e) {
            LOGGER.debug("Ожидаемое исключение при добавлении трека с тем же id", e);
        }

        LOGGER.info("Итоговое состояние плейлиста: {}", playlist);
    }
}

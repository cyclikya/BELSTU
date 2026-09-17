package music.service;

import music.model.Playlist;
import music.model.Track;
import music.model.User;
import music.repository.InMemoryPlaylistRepository;
import music.repository.PlaylistRepository;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;

public class PlaylistServiceTest {

    private PlaylistService service;
    private Playlist playlist;

    @BeforeEach
    void setUp() {
        PlaylistRepository repository = new InMemoryPlaylistRepository();
        service = new PlaylistService(repository);   // Dependency Injection в тесте
        User user = new User(1, "Виолетта");
        playlist = service.createPlaylist(1, "хайп", user);
    }

    // успех
    @Test
    void shouldAddTrackToPlaylist() {
        Track track = new Track(1, "Днями и ночами", "Бушидо Джо");

        service.addTrackToPlaylist(playlist.getId(), track);

        assertEquals(1, playlist.getTracks().size());
        assertTrue(playlist.hasTrack(1));
    }

    // нет плейлиста
    @Test
    void shouldThrowWhenPlaylistNotFound() {
        Track track = new Track(2, "Колыбельная", "Мильковский");

        assertThrows(IllegalArgumentException.class,
                () -> service.addTrackToPlaylist(999, track));
    }

    // дублирование трека(индивидуальное задание)
    @Test
    void shouldThrowWhenTrackIsDuplicate() {
        Track track = new Track(1, "Днями и ночами", "Бушидо Джо");
        service.addTrackToPlaylist(playlist.getId(), track);

        // другой объект, но тот же id
        Track duplicate = new Track(1, "Другое название", "Другой исполнитель");

        assertThrows(IllegalStateException.class,
                () -> service.addTrackToPlaylist(playlist.getId(), duplicate));
        assertEquals(1, playlist.getTracks().size());
    }
}
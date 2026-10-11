package music.service;

import music.model.Playlist;
import music.model.Track;
import music.model.User;
import music.repository.InMemoryPlaylistRepository;
import music.repository.PlaylistRepository;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;

public class PlaylistServiceSpec {

    private PlaylistRepository repository;
    private PlaylistService service;
    private User user;
    private Playlist playlist;

    @BeforeEach
    void setUp() {
        repository = new InMemoryPlaylistRepository();
        service = new PlaylistService(repository);
        user = new User(1, "Виолетта");
        playlist = service.createPlaylist(1, "хайп", user);
    }

    // Новый плейлист создаётся пустым
    @Test
    void newPlaylistShouldBeEmpty() {
        assertEquals(0, playlist.getTracks().size());
    }

    // createPlaylist не только создаёт объект, но и сохраняет его в репозиторий
    @Test
    void createdPlaylistShouldBeSavedInRepository() {
        Playlist found = repository.findById(1);

        assertNotNull(found);
        assertEquals("хайп", found.getName());
        assertEquals("Виолетта", found.getOwner().getName());
    }

    // Разные треки добавляются свободно
    @Test
    void shouldAddSeveralDifferentTracks() {
        service.addTrackToPlaylist(1, new Track(1, "Днями и ночами", "Бушидо Джо"));
        service.addTrackToPlaylist(1, new Track(2, "Колыбельная", "Мильковский"));
        service.addTrackToPlaylist(1, new Track(3, "Птичка", "Монеточка"));

        assertEquals(3, playlist.getTracks().size());
    }

    // Запрет на дубликат действует внутри одного плейлиста, а не глобально
    @Test
    void sameTrackMayBeAddedToDifferentPlaylists() {
        Playlist another = service.createPlaylist(2, "для учёбы", user);
        Track track = new Track(1, "Днями и ночами", "Бушидо Джо");

        service.addTrackToPlaylist(1, track);
        service.addTrackToPlaylist(2, track);

        assertTrue(playlist.hasTrack(1));
        assertTrue(another.hasTrack(1));
    }

    // getTracks отдаёт копию, которую нельзя менять в обход сервиса
    @Test
    void getTracksShouldReturnUnmodifiableCopy() {
        Track track = new Track(1, "Днями и ночами", "Бушидо Джо");
        service.addTrackToPlaylist(1, track);

        assertThrows(UnsupportedOperationException.class,
                () -> playlist.getTracks().add(track));
    }
}
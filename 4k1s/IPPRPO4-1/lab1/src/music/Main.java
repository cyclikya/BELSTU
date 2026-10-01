package music;

import music.model.Playlist;
import music.model.Track;
import music.model.User;
import music.repository.InMemoryPlaylistRepository;
import music.repository.PlaylistRepository;
import music.service.PlaylistService;

import static java.lang.System.out;

public class Main {
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
            out.println("Ошибка: " + e.getMessage());
        }

        Track track1Copy = new Track(1, "Днями и ночами", "Бушидо Джо");
        try {
            service.addTrackToPlaylist(1, track1Copy);
        } catch (IllegalStateException e) {
            out.println("Ошибка: " + e.getMessage());
        }

        out.println(playlist);
    }
}
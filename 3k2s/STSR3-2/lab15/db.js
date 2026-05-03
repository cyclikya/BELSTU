const { MongoClient } = require('mongodb');

const uri = 'mongodb+srv://admin:admin@lab15.ehijg0w.mongodb.net/BSTU?retryWrites=true&w=majority&appName=Lab15';

async function run() {
    const client = new MongoClient(uri);

    try {
        await client.connect();
        console.log('Подключение к MongoDB Atlas успешно');

        const database = client.db('BSTU');

        const faculties = database.collection('faculty');
        const pulpits = database.collection('pulpit');

        // Чтобы при повторном запуске не было дублей, сначала очищаем коллекции
        await faculties.deleteMany({});
        await pulpits.deleteMany({});

        // Заполнение коллекции faculty
        await faculties.insertMany([
            {
                faculty: 'ИТ',
                faculty_name: 'Информационных технологий'
            },
            {
                faculty: 'ИЭ',
                faculty_name: 'Инженерно-экономический'
            },
            {
                faculty: 'ЛХФ',
                faculty_name: 'Лесохозяйственный факультет'
            }
        ]);

        // Заполнение коллекции pulpit
        await pulpits.insertMany([
            {
                pulpit: 'ИСиТ',
                pulpit_name: 'Информационных систем и технологий',
                faculty: 'ИТ'
            },
            {
                pulpit: 'ПИ',
                pulpit_name: 'Программной инженерии',
                faculty: 'ИТ'
            },
            {
                pulpit: 'ЭТиМ',
                pulpit_name: 'Экономической теории и маркетинга',
                faculty: 'ИЭ'
            },
            {
                pulpit: 'ЛКиП',
                pulpit_name: 'Лесных культур и почвоведения',
                faculty: 'ЛХФ'
            }
        ]);

        console.log('Коллекции faculty и pulpit успешно заполнены');
    } catch (error) {
        console.error('Ошибка:', error);
    } finally {
        await client.close();
        console.log('Подключение закрыто');
    }
}

run();
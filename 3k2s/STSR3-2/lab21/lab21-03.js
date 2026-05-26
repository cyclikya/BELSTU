const express = require('express');
const session = require('express-session');

const users = require('./users.json');

const app = express();
const PORT = 3000;

app.use(express.urlencoded({ extended: true }));

app.use(session({
    secret: 'lab21-secret-key',
    resave: false,
    saveUninitialized: false
}));

function formsAuthMiddleware(req, res, next) {
    if (req.session && req.session.user) {
        req.isAuthenticated = true;
    } else {
        req.isAuthenticated = false;
    }

    next();
}

app.use(formsAuthMiddleware);

app.get('/login', (req, res) => {
    res.send(`
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="UTF-8">
            <title>Login</title>
        </head>
        <body>
            <h2>Forms authentication</h2>

            <form method="POST" action="/login">
                <label>Username:</label>
                <input type="text" name="username" required>
                <br><br>

                <label>Password:</label>
                <input type="password" name="password" required>
                <br><br>

                <button type="submit">Login</button>
            </form>
        </body>
        </html>
    `);
});

app.post('/login', (req, res) => {
    const { username, password } = req.body;
    const user = users[username];

    if (!user || user.password !== password) {
        return res.status(401).send(`
            <h2>Authentication failed</h2>
            <a href="/login">Try again</a>
        `);
    }

    req.session.user = username;
    res.redirect('/resource');
});

app.get('/logout', (req, res) => {
    req.session.destroy(() => {
        res.redirect('/login');
    });
});

app.get('/resource', (req, res) => {
    if (!req.isAuthenticated) {
        return res.redirect('/login');
    }

    res.send('RESOURCE');
});

app.use((req, res) => {
    res.status(404).send('404 Not Found');
});

app.listen(PORT, () => {
    console.log(`21-03 FORMS auth server started on http://localhost:${PORT}/resource`);
});
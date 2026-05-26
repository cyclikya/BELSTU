const express = require('express');
const passport = require('passport');
const BasicStrategy = require('passport-http').BasicStrategy;

const users = require('./users.json');

const app = express();
const PORT = 3000;

passport.use(new BasicStrategy((username, password, done) => {
    const user = users[username];

    if (!user || user.password !== password) {
        return done(null, false);
    }

    return done(null, { username });
}));

app.use(passport.initialize());

app.get('/login',
    passport.authenticate('basic', { session: false }),
    (req, res) => {
        res.redirect('/resource');
    }
);

app.get('/logout', (req, res) => {
    res.set('WWW-Authenticate', 'Basic realm="Login required"');
    res.status(401).send('Logout completed. Close the browser or clear authorization data.');
});

app.get('/resource', (req, res, next) => {
    if (!req.headers.authorization) {
        return res.redirect('/login');
    }

    passport.authenticate('basic', { session: false }, (err, user) => {
        if (err) {
            return next(err);
        }

        if (!user) {
            return res.redirect('/login');
        }

        res.send('RESOURCE');
    })(req, res, next);
});

app.use((req, res) => {
    res.status(404).send('404 Not Found');
});

app.listen(PORT, () => {
    console.log(`21-01 BASIC auth server started on http://localhost:${PORT}/resource`);
});
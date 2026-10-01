const url = '/api/Save-JSON';

function readBody() {
    return {
        op: document.getElementById('op').value,
        x: Number(document.getElementById('x').value),
        y: Number(document.getElementById('y').value)
    };
}

function show(status, text) {
    document.getElementById('status').textContent = status;
    document.getElementById('result').textContent = text;
}

async function send(method, body) {
    const options = { method: method };

    if (body) {
        options.headers = { 'Content-Type': 'application/json' };
        options.body = JSON.stringify(body);
    }

    try {
        const response = await fetch(url, options);
        const data = await response.json();
        show(response.status, JSON.stringify(data, null, 4));
    } catch (error) {
        show('ошибка', String(error));
    }
}

document.getElementById('btnGet').onclick = function () {
    send('GET');
};

document.getElementById('btnPost').onclick = function () {
    send('POST', readBody());
};

document.getElementById('btnPut').onclick = function () {
    send('PUT', readBody());
};

document.getElementById('btnDelete').onclick = function () {
    send('DELETE');
};

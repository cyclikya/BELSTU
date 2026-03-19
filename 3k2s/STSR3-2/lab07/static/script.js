// JSON
fetch('/data.json')
    .then(res => res.json())
    .then(data => {
        const div = document.createElement('div');
        div.innerHTML = `
            <h2>JSON данные</h2>
            <p>${data.title}</p>
            <p>${data.student.name} (${data.student.group})</p>
        `;
        document.body.appendChild(div);
    });

// XML
fetch('/data.xml')
    .then(res => res.text())
    .then(str => {
        const parser = new DOMParser();
        const xml = parser.parseFromString(str, "application/xml");

        const title = xml.querySelector("title").textContent;

        const div = document.createElement('div');
        div.innerHTML = `
            <h2>XML данные</h2>
            <p>${title}</p>
        `;
        document.body.appendChild(div);
    });
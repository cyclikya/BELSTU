const http = require("http");

const xmlData = `<?xml version="1.0" encoding="UTF-8"?>
<request id="28">
  <x value="1"/>
  <x value="2"/>
  <x value="3"/>
  <m value="a"/>
  <m value="b"/>
  <m value="c"/>
</request>`;

const req = http.request(
  "http://localhost:5000/task05",
  {
    method: "POST",
    headers: {
      "Content-Type": "application/xml",
      "Content-Length": Buffer.byteLength(xmlData)
    }
  },
  (res) => {
    const chunks = [];
    res.on("data", (chunk) => chunks.push(chunk));
    res.on("end", () => {
      const body = Buffer.concat(chunks).toString("utf8");
      console.log("Status:", res.statusCode, res.statusMessage);
      console.log("Body:", body);
    });
  }
);

req.on("error", (err) => {
  console.error("Request error:", err.message);
});

req.write(xmlData);
req.end();

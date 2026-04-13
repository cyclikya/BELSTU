const http = require("http");
const fs = require("fs");
const path = require("path");

const filePath = path.join(__dirname, "MyFile.png");
const fileData = fs.readFileSync(filePath);
const boundary = "----LAB09BOUNDARYPNG";

const head = Buffer.from(
  `--${boundary}\r\n` +
    `Content-Disposition: form-data; name="file"; filename="MyFile.png"\r\n` +
    `Content-Type: image/png\r\n\r\n`,
  "utf8"
);
const tail = Buffer.from(`\r\n--${boundary}--\r\n`, "utf8");
const body = Buffer.concat([head, fileData, tail]);

const req = http.request(
  "http://localhost:5000/task07",
  {
    method: "POST",
    headers: {
      "Content-Type": `multipart/form-data; boundary=${boundary}`,
      "Content-Length": body.length
    }
  },
  (res) => {
    const chunks = [];
    res.on("data", (chunk) => chunks.push(chunk));
    res.on("end", () => {
      const text = Buffer.concat(chunks).toString("utf8");
      console.log("Status:", res.statusCode, res.statusMessage);
      console.log("Body:", text);
    });
  }
);

req.on("error", (err) => {
  console.error("Request error:", err.message);
});

req.write(body);
req.end();

const http = require("http");
const fs = require("fs");
const path = require("path");

const outputDir = path.join(__dirname, "downloads");
if (!fs.existsSync(outputDir)) {
  fs.mkdirSync(outputDir, { recursive: true });
}

const outFile = path.join(outputDir, "MyFileFromServer.txt");

http.get("http://localhost:5000/task08/download", (res) => {
  const chunks = [];
  res.on("data", (chunk) => chunks.push(chunk));
  res.on("end", () => {
    const data = Buffer.concat(chunks);
    fs.writeFileSync(outFile, data);
    console.log("Status:", res.statusCode, res.statusMessage);
    console.log("Saved file:", outFile);
    console.log("Size:", data.length);
  });
}).on("error", (err) => {
  console.error("Request error:", err.message);
});

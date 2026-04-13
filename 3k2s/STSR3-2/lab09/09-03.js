const http = require("http");

const data = "x=7&y=5&s=hello";

const req = http.request(
  "http://localhost:5000/task03",
  {
    method: "POST",
    headers: {
      "Content-Type": "application/x-www-form-urlencoded",
      "Content-Length": Buffer.byteLength(data)
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

req.write(data);
req.end();

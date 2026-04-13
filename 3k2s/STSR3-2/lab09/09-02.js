const http = require("http");

http.get("http://localhost:5000/task02?x=10&y=4", (res) => {
  const chunks = [];
  res.on("data", (chunk) => chunks.push(chunk));
  res.on("end", () => {
    const body = Buffer.concat(chunks).toString("utf8");
    console.log("Status:", res.statusCode, res.statusMessage);
    console.log("Body:", body);
  });
}).on("error", (err) => {
  console.error("Request error:", err.message);
});

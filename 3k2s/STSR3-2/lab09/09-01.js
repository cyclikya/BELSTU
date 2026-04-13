const http = require("http");

http.get("http://localhost:5000/task01", (res) => {
  const serverIp = res.socket ? res.socket.remoteAddress : "unknown";
  const serverPort = res.socket ? res.socket.remotePort : "unknown";
  const chunks = [];
  res.on("data", (chunk) => chunks.push(chunk));
  res.on("end", () => {
    const body = Buffer.concat(chunks).toString("utf8");
    console.log("Status code:", res.statusCode);
    console.log("Status message:", res.statusMessage);
    console.log("Server IP:", serverIp);
    console.log("Server port:", serverPort);
    console.log("Body:", body);
  });
}).on("error", (err) => {
  console.error("Request error:", err.message);
});

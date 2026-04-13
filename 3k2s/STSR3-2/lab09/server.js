const http = require("http");
const fs = require("fs");
const path = require("path");

const PORT = 5000;
const UPLOAD_DIR = path.join(__dirname, "uploads");
const MY_TEXT_FILE = path.join(__dirname, "MyFile.txt");

if (!fs.existsSync(UPLOAD_DIR)) {
  fs.mkdirSync(UPLOAD_DIR, { recursive: true });
}

function readBody(req, callback) {
  const chunks = [];
  req.on("data", (chunk) => chunks.push(chunk));
  req.on("end", () => callback(Buffer.concat(chunks)));
}

function parseXmlRequest(xmlText) {
  const requestIdMatch = xmlText.match(/<request[^>]*id="([^"]+)"/i);
  const requestId = requestIdMatch ? requestIdMatch[1] : "0";

  const xMatches = [...xmlText.matchAll(/<x[^>]*value="([^"]+)"/gi)];
  const mMatches = [...xmlText.matchAll(/<m[^>]*value="([^"]+)"/gi)];

  const sumX = xMatches.reduce((sum, m) => sum + Number(m[1]), 0);
  const concatM = mMatches.map((m) => m[1]).join("");

  const responseId = Math.floor(Math.random() * 1000);
  return `<?xml version="1.0" encoding="UTF-8"?>
<response id="${responseId}" request="${requestId}">
  <sum element="x" result="${sumX}" />
  <concat element="m" result="${concatM}" />
</response>`;
}

function saveMultipartFile(req, bodyBuffer) {
  const contentType = req.headers["content-type"] || "";
  const boundaryMatch = contentType.match(/boundary=(.+)$/);
  if (!boundaryMatch) {
    return { error: "Boundary not found in Content-Type" };
  }

  const boundary = boundaryMatch[1];
  const headersEnd = bodyBuffer.indexOf(Buffer.from("\r\n\r\n"));
  if (headersEnd < 0) {
    return { error: "Invalid multipart body" };
  }

  const partHeaders = bodyBuffer.slice(0, headersEnd).toString("utf8");
  const nameMatch = partHeaders.match(/filename="([^"]+)"/i);
  const fileName = nameMatch ? path.basename(nameMatch[1]) : "uploaded.bin";

  const fileStart = headersEnd + 4;
  const closingBoundary = Buffer.from(`\r\n--${boundary}--`);
  const fileEnd = bodyBuffer.indexOf(closingBoundary);
  if (fileEnd < 0) {
    return { error: "Closing boundary not found" };
  }

  const fileContent = bodyBuffer.slice(fileStart, fileEnd);
  const savePath = path.join(UPLOAD_DIR, fileName);
  fs.writeFileSync(savePath, fileContent);
  return { fileName, size: fileContent.length, savePath };
}

const server = http.createServer((req, res) => {
  const url = new URL(req.url, `http://${req.headers.host}`);

  if (req.method === "GET" && url.pathname === "/task01") {
    res.writeHead(200, { "Content-Type": "text/plain; charset=utf-8" });
    res.end("Task 01 server response");
    return;
  }

  if (req.method === "GET" && url.pathname === "/task02") {
    const x = Number(url.searchParams.get("x"));
    const y = Number(url.searchParams.get("y"));
    const body = `x=${x}, y=${y}, x+y=${x + y}, x-y=${x - y}, x*y=${x * y}`;
    res.writeHead(200, { "Content-Type": "text/plain; charset=utf-8" });
    res.end(body);
    return;
  }

  if (req.method === "POST" && url.pathname === "/task03") {
    readBody(req, (bodyBuffer) => {
      const params = new URLSearchParams(bodyBuffer.toString("utf8"));
      const x = Number(params.get("x"));
      const y = Number(params.get("y"));
      const s = params.get("s") || "";
      const body = `x+y=${x + y}, s=${s}`;
      res.writeHead(200, { "Content-Type": "text/plain; charset=utf-8" });
      res.end(body);
    });
    return;
  }

  if (req.method === "POST" && url.pathname === "/task04") {
    readBody(req, (bodyBuffer) => {
      const input = JSON.parse(bodyBuffer.toString("utf8"));
      const response = {
        __comment: "Response from lab08 task10 structure",
        x_plus_y: Number(input.x) + Number(input.y),
        Concatination_s_o: `${input.s}: ${input.o.surname}, ${input.o.name}`,
        Length_m: Array.isArray(input.m) ? input.m.length : 0
      };
      res.writeHead(200, { "Content-Type": "application/json; charset=utf-8" });
      res.end(JSON.stringify(response));
    });
    return;
  }

  if (req.method === "POST" && url.pathname === "/task05") {
    readBody(req, (bodyBuffer) => {
      const xmlResponse = parseXmlRequest(bodyBuffer.toString("utf8"));
      res.writeHead(200, { "Content-Type": "application/xml; charset=utf-8" });
      res.end(xmlResponse);
    });
    return;
  }

  if (req.method === "POST" && (url.pathname === "/task06" || url.pathname === "/task07")) {
    readBody(req, (bodyBuffer) => {
      const result = saveMultipartFile(req, bodyBuffer);
      if (result.error) {
        res.writeHead(400, { "Content-Type": "text/plain; charset=utf-8" });
        res.end(result.error);
        return;
      }

      res.writeHead(200, { "Content-Type": "text/plain; charset=utf-8" });
      res.end(`File saved: ${result.fileName}, size=${result.size}`);
    });
    return;
  }

  if (req.method === "GET" && url.pathname === "/task08/download") {
    const data = fs.readFileSync(MY_TEXT_FILE);
    res.writeHead(200, {
      "Content-Type": "application/octet-stream",
      "Content-Disposition": "attachment; filename=\"MyFileFromServer.txt\"",
      "Content-Length": data.length
    });
    res.end(data);
    return;
  }

  res.writeHead(404, { "Content-Type": "text/plain; charset=utf-8" });
  res.end("Route not found");
});

server.listen(PORT, () => {
  console.log(`Server started at http://localhost:${PORT}`);
});

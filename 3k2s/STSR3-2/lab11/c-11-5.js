const WebSocket = require('ws');
const rpcWS = require('rpc-websockets').Client;

const ws = new rpcWS('ws://localhost:4000');

ws.on('open', () => {
  ws.call('square', [3]).then((r) => { console.log('square(3):', r); }).catch((e) => { console.error('square(3) error:', e); });
  ws.call('square', [5, 4]).then((r) => { console.log('square(5,4):', r); }).catch((e) => { console.error('square(5,4) error:', e); });
  ws.call('sum', [2]).then((r) => { console.log('sum(2):', r); }).catch((e) => { console.error('sum(2) error:', e); });
  ws.call('sum', [2, 4, 6, 8, 10]).then((r) => { console.log('sum(2,4,6,8,10):', r); }).catch((e) => { console.error('sum error:', e); });
  ws.call('mul', [3]).then((r) => { console.log('mul(3):', r); }).catch((e) => { console.error('mul(3) error:', e); });
  ws.call('mul', [3, 5, 7, 9, 11, 13]).then((r) => { console.log('mul(3,5,7,9,11,13):', r); }).catch((e) => { console.error('mul error:', e); });
  // Protected методы fib и fact не вызываются в базовом клиенте c-11-5.js

});

ws.on('error', (e) => {
  console.error('Error:', e);
});

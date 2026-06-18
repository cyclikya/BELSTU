import { createClient } from 'redis';

const client = createClient({ url: 'redis://default:j8COEzsOzQnwhvpwPf4KFvADdKeuwQLj@redis-11725.c74.us-east-1-4.ec2.cloud.redislabs.com:11725' });

client.on('error', err => {
  console.log('Redis Client Error:', err);
});

await client.connect();

// замеряет время выполнения 10000 операций
async function test(operation) {
  const start = Date.now();
  const promises = [];
  for (let n = 1; n <= 10000; n++) {
    promises.push(operation(n));
  }
  await Promise.all(promises);
  return Date.now() - start;
}

const setTime = await test(n => client.set(String(n), `set${n}`));
const getTime = await test(n => client.get(String(n)));
const delTime = await test(n => client.del(String(n)));

console.table([
  { operation: 'set', time_ms: setTime },
  { operation: 'get', time_ms: getTime },
  { operation: 'del', time_ms: delTime }
]);

await client.quit();
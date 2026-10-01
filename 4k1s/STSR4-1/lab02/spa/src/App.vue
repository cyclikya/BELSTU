<script setup>
    import { ref } from 'vue';

    const url = '/api/Save-JSON';

    const op = ref('sub');
    const x = ref(3);
    const y = ref(5);
    const status = ref('—');
    const result = ref('Нажмите кнопку');

    async function send(method, withBody) {
        const options = { method: method };

        if (withBody) {
            options.headers = { 'Content-Type': 'application/json' };
            options.body = JSON.stringify({
                op: op.value,
                x: Number(x.value),
                y: Number(y.value)
            });
        }

        try {
            const response = await fetch(url, options);
            const data = await response.json();
            status.value = response.status;
            result.value = JSON.stringify(data, null, 4);
        } catch (error) {
            status.value = 'ошибка';
            result.value = String(error);
        }
    }
</script>

<template>
    <h1>TDWA02-02</h1>

    <div class="form">
        <label>op
            <select v-model="op">
                <option value="add">add</option>
                <option value="sub">sub</option>
                <option value="mul">mul</option>
                <option value="div">div</option>
            </select>
        </label>
        <label>x
            <input v-model="x" type="number">
        </label>
        <label>y
            <input v-model="y" type="number">
        </label>
    </div>

    <div class="buttons">
        <button @click="send('GET', false)">GET</button>
        <button @click="send('POST', true)">POST</button>
        <button @click="send('PUT', true)">PUT</button>
        <button @click="send('DELETE', false)">DELETE</button>
    </div>

    <p class="status">Статус ответа: <span id="status">{{ status }}</span></p>
    <pre class="result">{{ result }}</pre>
</template>

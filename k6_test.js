import http from 'k6/http';
import { sleep, check } from 'k6';
import { randomString } from 'https://jslib.k6.io/k6-utils/1.2.0/index.js';

export const options = {
    insecureSkipTLSVerify: true,
    stages: [
        { duration: '30s', target: 10 },  // 10 usuários por 30 segundos
        { duration: '1m', target: 50 },   // 50 usuários por 1 minuto
        { duration: '2m', target: 100 },  // 100 usuários por 2 minutos
        { duration: '1m', target: 50 },   // 50 usuários por 1 minuto
        { duration: '30s', target: 10 },  // Reduz para 10 usuários por 30 segundos
        // { duration: '1m', target: 400 }

    ],
};

let url = 'http://localhost:9999';

export default function () {

    var result = createPessoa();
    searchPessoa(result);
    searchString();
    contagemPessoas();

}

function createPessoa() {
    const parameter = {
        "apelido": randomString(8),
        "nome": randomString(10, 'aeioubcdfghijpqrstuv'),
        "nascimento": "2023-08-09",
        "stacks": ["c#", "go"]
    }

    const res = http.post(url + '/pessoas', JSON.stringify(parameter), {
        headers: { 'Content-Type': 'application/json' },
    });

    check(res, {
        'Criação de pessoas - Status 201': (r) => r.status === 201,
    });

    sleep(1);

    return res;
}

function searchPessoa(header) {

    if (header.status == 201) {
        var url = header.headers['Location'];
        var res = http.get(url);

        check(res, {
            'Busca de pessoas - Status 200': (r) => r.status === 200,
        });
    }
    sleep(1);
}

function searchString() {

    var res = http.get(url + '/pessoas/?t=go');
    check(res, {
        'Busca de pessoas string - Status 200': (r) => r.status === 200,
    });

    sleep(1);
}

function contagemPessoas() {

    var res = http.get(url + '/contagem-pessoas');
    check(res, {
        'Busca contagem pessoa - Status 200': (r) => r.status === 200,
    });

    sleep(1);
}

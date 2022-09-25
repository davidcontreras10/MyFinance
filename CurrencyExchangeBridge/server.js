const https = require('https');
const express = require('express');
const app = express();
const htmlEntities = require('html-entities');
const decoder = htmlEntities.decode;
//var path = require('path');
require('dotenv').config();


const router = express.Router();

app.get('/currency', function (expReq, expRes) {
    const inicial = expReq.query.FechaInicio;
    const final = expReq.query.FechaFinal;
    const indicador = expReq.query.Indicador;
    if (inicial && final && indicador) {
        const testApi = `https://gee.bccr.fi.cr/Indicadores/Suscripciones/WS/wsindicadoreseconomicos.asmx/ObtenerIndicadoresEconomicosXML?Indicador=${indicador}&FechaInicio=${inicial}&FechaFinal=${final}&Nombre=David&SubNiveles=s&CorreoElectronico=dcontre10@gmail.com&Token=1IO9ALGN0O`;
        console.log(`Requesting: ${testApi}`);
        https.get(testApi, (resp) => {
            let data = null;

            // A chunk of data has been received.
            resp.on('data', (chunk) => {
                data = chunk;
            });

            // The whole response has been received. Print out the result.
            resp.on('end', () => {
                const xmlString = decoder(data.toString());
                expRes.type('application/xml');
                expRes.status(200).send(xmlString);
            });

        }).on("error", (err) => {
            console.log("Error: " + err.message);
            throw err;
        });
    }
    else {
        expRes.statusMessage = "Invalid parameters";
        expRes.status(400).end();
    }
});

app.get('/', function (req, res) {
    res.json('index result');
});

const PORT = process.env.PORT || 8080;

app.listen(PORT, () => {
    console.log(`MYF App listening on ${PORT}`)
});

module.exports = router;
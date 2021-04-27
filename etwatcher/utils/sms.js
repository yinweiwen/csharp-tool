/**
 * Created by PengLing on 2017/8/3
 */
'use strict';

const request = require('superagent');

let batchSend = async function (params, config) {
    const defaultConfig = require('../config').push.sms;
    config = config || defaultConfig;
    try {
        if (params && params.receivers && params.title && params.content) {
            const apikey = config.apikey;
            const url = 'https://sms.yunpian.com/v2/sms/batch_send.json';
            let response = await request.post(url)
                .set('Content-Type', 'application/x-www-form-urlencoded')
                .send('apikey=' + apikey)
                .send('mobile=' + params.receivers)
                .send('text=' + `${params.title}${params.content}`);
            const { status, body } = response;
            if (status === 200) {
                const { total_count, data } = body;
                console.log("send sms count "+total_count);
            } else {
                console.log("send sms err");
            }
        }
    } catch (err) {
        console.log(err);
    }
};

module.exports = {
    batchSend
};

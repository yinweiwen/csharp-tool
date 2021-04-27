/**
 * Created by PengLing on 2017/8/3
 */
'use strict';

const nodemailer = require('nodemailer');

module.exports.send = function (params, config) {
    const defaultConfig = require('../config').push.email;
    config = config || defaultConfig;
    return new Promise(async (resolve, reject) => {
        try {
            if (!config || config.enabled !== true) {
                return resolve();
            }
            if (params && params.receivers && params.receivers.length && params.title && params.content) {
                let transporter = nodemailer.createTransport({
                    host: config.host,
                    port: config.port,
                    secure: true,
                    auth: {
                        user: config.sender.address,
                        pass: config.sender.password
                    }
                });
                let mailOptions = {
                    from: `"${config.sender.name}"<${config.sender.address}>`, // sender address
                    to: params.receivers,
                    subject: params.title,
                    html: params.content
                };
                let rslt = await transporter.sendMail(mailOptions);
                resolve(rslt);
            } else {
                throw 'params error.';
            }
        } catch (err) {
            reject(err);
        }
    });
};

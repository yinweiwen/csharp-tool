/**
 * Create by PengLing on 2017/8/24.
 */
'use strict';

const moment = require('moment');

let momentToDateTimeString = function (momentDT, format) {
    let utcFormat = 'YYYY-MM-DDTHH:mm:ss.SSSZ';
    let formatString = format || utcFormat;
    let datetimeString = formatString == utcFormat ? momentDT.toISOString() : momentDT.format(formatString);
    return datetimeString;
};

let getRangeByPeriod = function (period, format) {
    let formatString = format || 'YYYY-MM-DD HH:mm:ss.SSS';
    try {
        let range = null, start = null, end = null, momentDT = null;
        let now = momentToDateTimeString(moment(), formatString);
        switch (period) {
            case 'LAST_MONTH':
                momentDT = moment().subtract(1, 'months').startOf('month');
                start = momentToDateTimeString(momentDT, formatString);
                momentDT = moment().startOf('month');
                end = momentToDateTimeString(momentDT, formatString);
                range = { $gte: start, $lt: end };
                break;
            case 'THIS_MONTH':
                momentDT = moment().startOf('month');
                start = momentToDateTimeString(momentDT, formatString);
                range = { $between: [start, now] };
                break;
            case 'LAST_WEEK':
                momentDT = moment().subtract(1, 'weeks').startOf('isoWeek');
                start = momentToDateTimeString(momentDT, formatString);
                momentDT = moment().startOf('isoWeek');
                end = momentToDateTimeString(momentDT, formatString);
                range = { $gte: start, $lt: end };
                break;
            case 'THIS_WEEK':
                momentDT = moment().startOf('isoWeek');
                start = momentToDateTimeString(momentDT, formatString);
                range = { $between: [start, now] };
                break;
            case 'LAST_DAY':
                momentDT = moment().subtract(1, 'days').startOf('day');
                start = momentToDateTimeString(momentDT, formatString);
                momentDT = moment().startOf('day');
                end = momentToDateTimeString(momentDT, formatString);
                range = { $gte: start, $lt: end };
                break;
            case 'THIS_DAY':
                momentDT = moment().startOf('day');
                start = momentToDateTimeString(momentDT, formatString);
                range = { $between: [start, now] };
                break;
            default:
                break;
        };
        return range;
    } catch (err) {
        throw err;
    }
};

let getRange = function (operate, number, period, format, upToNow = false, inquery = true) {
    if (!['add', 'subtract'].includes(operate) || !['year', 'month', 'isoWeek', 'week', 'day'].includes(period)) return null;

    let formatString = format || 'YYYY-MM-DD HH:mm:ss.SSS';
    try {
        let range = null, start = null, end = null
        let periodX = period == 'isoWeek' ? 'weeks' : `${period}s`;
        let now = moment().format(formatString);
        switch (operate) {
            case 'subtract':
                start = moment().subtract(number, periodX).startOf(period).format(formatString);
                if (upToNow) {
                    range = inquery ? { $between: [start, now] } : { start, end: now };
                } else {
                    end = inquery
                        ? moment().subtract(number - 1, periodX).startOf(period).format(formatString)
                        : moment().subtract(number, periodX).endOf(period).format(formatString);
                    range = inquery ? { $gte: start, $lt: end } : { start, end };
                }
                break;
            case 'add':
                start = moment().add(number, periodX).startOf(period).format(formatString);
                if (upToNow) {
                    range = inquery ? { $between: [start, now] } : { start, end: now };
                } else {
                    end = inquery
                        ? moment().add(number + 1, periodX).startOf(period).format(formatString)
                        : moment().add(number, periodX).endOf(period).format(formatString);
                    range = inquery ? { $gte: start, $lt: end } : { start, end };
                }
                break;
            default:
                start = moment().startOf('day').format(formatString);
                range = inquery ? { $between: [start, now] } : { start, end: now };
                break;
        };
        return range;
    } catch (err) {
        throw err;
    }
};

/**
 *  将datetime转换为UTC时间格式
 * @param {datetimeString || Moment} datetime 时间字符串或者Moment对象
 * @returns {string} "YYYY-MM-DDTHH:mm:ss.SSSZ"格式的时间字符串
 */
let toUTCString = function (datetime) {
    let utcString = moment(datetime).toISOString();
    return utcString;
};

/**
 *  将datetime转换为中国标准时间格式，时间默认精确到秒
 * @param {datetimeString || Moment} datetime 时间字符串或者Moment对象
 * @returns {string} "YYYY-MM-DD HH:mm:ss"格式的时间字符串
 */
let toCSTString = function (datetime, format) {
    let formatString = format || 'YYYY-MM-DD HH:mm:ss';
    let cstString = moment(datetime).format(formatString);
    return cstString;
};

module.exports = {
    entry: (app, router, opts) => {
        const timer = {
            getRangeByPeriod,
            getRange,
            toUTCString,
            toCSTString
        };

        app.fs = app.fs || {};
        app.fs.timer = timer;
    }
};

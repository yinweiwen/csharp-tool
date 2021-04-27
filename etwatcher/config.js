
// const dev = process.env.NODE_ENV == 'development';
const dev=true;

module.exports={
    jobs:[
        {cmd:'echo root123| sudo -S yarn application -list',check:/application_[\s\S]*RUNNING/i,cron:'*/30 * * * * *',desc:'ET进程疑似掉线',disable:dev},
        {cmd:'more .\\test.txt', check:/application_[\s\S]*RUNNING/i,cron:'*/30 * * * * *',desc:'测试描述和消息推送',disable:!dev},
        {
            // cmd:'.\\search_latest.sh',
            cmd:"more .\\test2.json",
            t_check:/"collect_time" : "((\w|-|:|.)+)"/ig,
            t_less:20,
            cron:'*/10 * * * * *',
            desc:'知物云数据疑似中断(近20分钟未查找到任何数据)',
            disable:!dev
        }
    ],
    push: {
        email: {
            enabled: true,
            host: 'smtp.exmail.qq.com',
            port: 465,
            sender: {
                name: '告警',
                address: '',
                password: ''
            },
            receivers:[
                {email:''}
            ]
        },
        sms: {
            enabled: false,
            apikey: '',
            receivers:[
                {phone:''}
            ]
        }
    },
};


// const dev = process.env.NODE_ENV == 'development';
const dev=true;

module.exports={
    jobs:[
        {cmd:'echo root123| sudo -S yarn application -list',check:/application_[\s\S]*RUNNING/i,cron:'*/30 * * * * *',desc:'ET进程疑似掉线',disable:dev},
        {
            cmd:"kubectl logs -n savoir -l app=recv2process --tail 1",
            t_check: /Time:(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d+)/i,
            t_format:"yyyy-MM-DD HH:mm:ss.fff",
            t_less:20,
            cron:'*/30 * * * * *',
            desc:'知物云数据接收中断(近20分钟)',
            disable:dev
        },
        {
            cmd:".\\savoir_data_latest.sh",
            t_check:/"collect_time" : "((\w|-|:|.)+)"/ig,
            t_less:20,
            cron:'*/30 * * * * *',
            desc:'知物云主题数据疑似中断(近20分钟未查找到任何数据)',
            disable:dev
        },
        // 测试 ↓↓↓
        {cmd:'more .\\test.txt', check:/application_[\s\S]*RUNNING/i,cron:'*/30 1 * * * *',desc:'测试描述和消息推送',disable:!dev},
        {
            cmd:"more .\\test2.json",
            t_check:/"collect_time" : "((\w|-|:|.)+)"/ig,
            t_less:20,
            cron:'*/10 1 * * * *',
            desc:'[TEST]知物云主题数据疑似中断(近20分钟未查找到任何数据)',
            disable:!dev
        },
        {
            cmd:"more .\\test3.txt",
            t_check: /Time:(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d+)/i,
            t_format:"yyyy-MM-DD HH:mm:ss.fff",
            t_less:20,
            cron:'*/10 1 * * * *',
            desc:'[TEST]知物云数据接收中断(近20分钟)',
            disable:!dev
        },
        {
            cmd:"more .\\test4.txt",
            n_less: 100,
            cron:'*/10 * * * * *',
            desc:'[TEST]数字比较告警(近20分钟)',
            disable:!dev
        },
        {
            cmd:"ps aux | grep dac | sort -nr -k4 | head -n1 | awk '{print $6}'",
            n_less: 6291456,
            cron:'* */5 * * * *',
            desc:'DAC内存过高',
            disable:dev
        }
    ],
    push: {
        email: {
            enabled: true,
            host: 'smtp.exmail.qq.com',
            port: 465,
            sender: {
                name: '',
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

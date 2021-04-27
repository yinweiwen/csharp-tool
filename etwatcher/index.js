var sys=require('sys');
var exec=require('child_process').exec;
var cron = require('cron');
var pushConfig=require('./config.js').push;

const email=require('./utils/email');
const sms=require('./utils/sms');
const moment = require('moment');

require('./config.js').jobs.filter(j=>!j.disable).forEach(job => {
    job.check_status=true;
    var cronJob = cron.job(job.cron, function(){
        console.log('run cmd '+ job.cmd);

        dir = exec(job.cmd,function(err,stdout,stderr){
            if(err){
                console.log('cmd run err: '+err.message);
            }else{
                console.log('cmd run out: '+stdout);
                let res=check(job,stdout);
                if(!res){
                    console.log('*******CHECK RESULT*******');
                    console.log(job);
                    console.log('*******CHECK FAILED*******');
                    if(job.check_status){
                        job.check_status=false;
                        let msg={
                            title:"进程监控汇报",
                            content:job.desc
                        };
                        doPush(msg);
                    }
                }else{
                    console.log("check success!")
                    if(!job.check_status){
                        let msg={
                            title:"进程监控汇报",
                            content:'问题: ['+job.desc +'] 已经修复'
                        };
                        doPush(msg);
                    }
                    job.check_status=true;
                }
            }
        });

        dir.on('exit',function(code){
            console.log('exit code: '+code);
            if(code==0){
                
            }
        });
    }); 
    cronJob.start();
    
});

function check(job,stdout){
    if(job.check){
        return job.check.test(stdout);
    }
    if(job.t_check){
        if(!job.t_check.test(stdout)){
            console.log('not match t_check, is check format allright?')
            return true;
        }

        var flag=false;

        stdout.replace(job.t_check,function(){
            console.log(arguments[1]);
            var duration=moment.duration(moment().diff(moment(arguments[1])))
            var minutes=duration.asMinutes()
            console.log(minutes)
            flag= minutes<=job.t_less
        })
        setTimeout(() => {
            
        }, 1000);
        return flag;
    }
}

function doPush(msg){
    if(pushConfig.email.enabled){
        let params = {
            receivers: pushConfig.email.receivers.map(r => r.email),
            title: msg.title,
            content: msg.content
        };
        email.send(params);
    }
    if(pushConfig.sms.enabled){
        let params = {
            receivers: pushConfig.sms.receivers.map(r => r.phone),
            title: msg.title,
            content: msg.content
        };
        sms.batchSend(params);
    }
}



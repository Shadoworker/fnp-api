const CONSTANTS = {

    api_url : "http://192.168.1.12:1337/",
    api_path : "l3v3l/callback",
    playtix_base_url : `https://connect.playtix.team/oauth2/aus7e5j3kfGHKetdl5d7`,
    client_id : '0oa7e5jz4w9xy416F5d7',
    client_secret : 'tPHsuPBU_q3E9SQkEjV37q2wZZgQ8vf8jnRAVkpk',
    grant_url : function (_api_url:string, _callback_path:string, _code:string) 
    { 
        return `grant_type=authorization_code&redirect_uri=${_api_url}api/${_callback_path}&code=${_code}`
    },



}

export default CONSTANTS;
const CONSTANTS = {

    api_url : "http://localhost:1337/",
    api_path : "l3v3l/callback",

    playtix_base_url : `https://connect.playtix.team/oauth2/aus7e5j3kfGHKetdl5d7`,
    client_id : '0oa7e5jz4w9xy416F5d7',
    client_secret : 'tPHsuPBU_q3E9SQkEjV37q2wZZgQ8vf8jnRAVkpk',

    playtix_request_base_url : `https://connect.playtix.team/oauth2/aus3iwvbgi8x9IWi95d7`,
    requests_client_id : '0oa7e5i7g6VMFi83w5d7',
    requests_client_secret :'rdmqeOu8PbhDc3DxqXVnB1ghK9Miu8EfW4gJrArK',

    playtix_api_base_url : `https://api.playtix.team/`,

    player_id_test : "0d5356d5-1c67-4e6b-bc8f-30571af18382",

    grant_url : function (_api_url:string, _callback_path:string, _code:string) 
    { 
        return `grant_type=authorization_code&redirect_uri=${_api_url}api/${_callback_path}&code=${_code}`
    },



}

export default CONSTANTS;
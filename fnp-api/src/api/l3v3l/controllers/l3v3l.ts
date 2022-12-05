/**
 * l3v3l controller
 */

import { factories } from '@strapi/strapi'
import axios from 'axios'; 

import {btoa} from 'buffer';

const open = require("open")

const OktaJwtVerifier = require('@okta/jwt-verifier');


export default factories.createCoreController('api::l3v3l.l3v3l', ({ strapi }) =>  ({


    async callback(ctx) { // l3v3l MAIN CALLBACK : called by GET /callback

 
        var url_string = "http://192.168.1.12:1337/"+ ctx.originalUrl;

        var url = new URL(url_string);
        var code = url.searchParams.get("code");

        // console.log(code);

        let client_id = '0oa7e5jz4w9xy416F5d7';
        let client_secret = 'tPHsuPBU_q3E9SQkEjV37q2wZZgQ8vf8jnRAVkpk';

        const auth_header = btoa(`${client_id}:${client_secret}`);


        let config = {
          headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/x-www-form-urlencoded',
            Authorization: 'Basic '+ auth_header
          }
         }

        var body = "grant_type=authorization_code&redirect_uri=http%3A%2F%2F192.168.1.12%3A1337%2Fapi%2Fl3v3l%2Fcallback&code="+code;


        // Get Token
        try {

          const { data } = await axios.post(`https://connect.playtix.team/oauth2/aus7e5j3kfGHKetdl5d7/v1/token`, body, config)

          var accessTokenString = data.id_token;

          const oktaJwtVerifier = new OktaJwtVerifier({
            issuer: 'https://connect.playtix.team/oauth2/aus7e5j3kfGHKetdl5d7' // required
          });

          // Get Player_ID
          try {
            
              let jwt = await oktaJwtVerifier.verifyAccessToken(accessTokenString, "0oa7e5jz4w9xy416F5d7")

              // console.log(jwt)
              // Return values
              var claims = JSON.stringify(jwt.claims);
              console.log(claims)

              ctx.body = `
                <html>
                <head>
                <meta http-equiv=Content-Type content="text/html; charset=utf-8">
                <meta name="viewport" content="width=device-width, initial-scale=1" />

                </head>
                <style>
                  .box
                  {
                    display:flex; 
                    width:100%;
                    align-items:center;
                    justify-content:center;
                    flex-direction : column;
                  }
                  .title
                  {
                    font-family : Arial;
                  }
                  .btn {
                    display: block;
                    height: 25px;
                    background: #4E9CAF;
                    padding: 10px;
                    text-align: center;
                    border-radius: 5px;
                    color: white;
                    font-weight: bold;
                    line-height: 25px;
                    text-decoration:none;
                    font-family:Arial;
                    font-size:14px;
                    padding:8px 20px;
                  }
                </style>
                <body >
                <div class="box">
                <h3 class="title" >Connexion établie !</h3>
                <span>Votre token : <span style="font-weight:bold"> ${accessTokenString} </span> </span>
                <a id="btn" class="btn" href='fnpdl://signinLink?${claims}'>RETOURNER AU JEU</a>
                </div>
 
                </body>
                </html>
              `;
  
            } catch (err) {
              ctx.body = err;
            }
         

        } catch (err) {
          ctx.body = err;
        }
 


      },

    
      async getUserData(ctx) { // User data using token

 
        var url_string = "http://192.168.1.12:1337/"+ ctx.originalUrl;

        var url = new URL(url_string);
        var accessTokenString = url.searchParams.get("id_token");
      
 
        const oktaJwtVerifier = new OktaJwtVerifier({
          issuer: 'https://connect.playtix.team/oauth2/aus7e5j3kfGHKetdl5d7' // required
        });

        // Get Player_ID
        try {
          
            let jwt = await oktaJwtVerifier.verifyAccessToken(accessTokenString, "0oa7e5jz4w9xy416F5d7")

            // console.log(jwt)
            // Return values
            var claims = JSON.stringify(jwt.claims);
            console.log(claims)

            ctx.body = claims;

          } catch (err) {
            ctx.body = err;
          }
 


      },

    // Method 2: Wrapping a core action (leaves core logic in place)
    async find(ctx) {
      // some custom logic here
      ctx.query = { ...ctx.query, local: 'en' }
      
      // Calling the default core action
      const { data, meta } = await super.find(ctx);
  
      // some more custom logic
      meta.date = Date.now()
  
      return { data, meta };
    },
  
    // Method 3: Replacing a core action
    async findOne(ctx) {
      const { id } = ctx.params;
      const { query } = ctx;
  
      const entity = await strapi.service('api::l3v3l.l3v3l').findOne(id, query);
      const sanitizedEntity = await this.sanitizeOutput(entity, ctx);
  
      return this.transformResponse(sanitizedEntity);
    }
  }));
   
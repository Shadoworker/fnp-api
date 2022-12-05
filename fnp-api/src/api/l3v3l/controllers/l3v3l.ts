/**
 * l3v3l controller
 */

import { factories } from '@strapi/strapi'
import axios from 'axios'; 

import {btoa} from 'buffer';
import CONSTANTS from '../content-types/constants';
import view from '../content-types/view';

const open = require("open")

const OktaJwtVerifier = require('@okta/jwt-verifier');


export default factories.createCoreController('api::l3v3l.l3v3l', ({ strapi }) =>  ({


    async callback(ctx) { // l3v3l MAIN CALLBACK : called by GET /callback

 
        var url_string = CONSTANTS.api_url+ ctx.originalUrl;

        var url = new URL(url_string);
        var code = url.searchParams.get("code");

        // console.log(code);

        let client_id = CONSTANTS.client_id;
        let client_secret = CONSTANTS.client_secret;

        const auth_header = btoa(`${client_id}:${client_secret}`);


        let config = {
          headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/x-www-form-urlencoded',
            Authorization: 'Basic '+ auth_header
          }
         }

        var body = CONSTANTS.grant_url(CONSTANTS.api_url, CONSTANTS.api_path, code);
        
        // Get Token
        try {

          let _url = CONSTANTS.playtix_base_url+'/v1/token';
          const { data } = await axios.post(_url, body, config)

          var accessTokenString = data.id_token;

          const oktaJwtVerifier = new OktaJwtVerifier({
            issuer: CONSTANTS.playtix_base_url // required
          });

          // Get Player_ID
          try {
            
              let jwt = await oktaJwtVerifier.verifyAccessToken(accessTokenString, CONSTANTS.client_id)

              // console.log(jwt)
              // Return values
              var claims = JSON.stringify(jwt.claims);
              console.log(claims)

              ctx.body = view(accessTokenString, claims);
  
            } catch (err) {
              ctx.body = err;
            }
         

        } catch (err) {
          ctx.body = err;
        }
 


      },

    
      async getUserData(ctx) { // User data using token

 
        var url_string = CONSTANTS.api_url+ ctx.originalUrl;

        var url = new URL(url_string);
        var accessTokenString = url.searchParams.get("id_token");
      
 
        const oktaJwtVerifier = new OktaJwtVerifier({
          issuer: CONSTANTS.playtix_base_url // required
        });

        // Get Player_ID
        try {
          
            let jwt = await oktaJwtVerifier.verifyAccessToken(accessTokenString, CONSTANTS.client_id)

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
   
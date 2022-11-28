/**
 * l3v3l controller
 */

import { factories } from '@strapi/strapi'
import axios from 'axios';

import {btoa} from 'buffer';


export default factories.createCoreController('api::l3v3l.l3v3l', ({ strapi }) =>  ({
    // Method 1: Creating an entirely custom action
    async exampleAction(ctx) {
      try {
        ctx.body = 'ok';
      } catch (err) {
        ctx.body = err;
      }
    },
  
    async callback(ctx) { // called by GET /callback

        ctx.body = 'Link generated !'; // we could also send a JSON

        // `https://api.github.com/users`
        console.log("ctx.params")
        var url_string = "http://localhost:1337/"+ ctx.originalUrl;

        var url = new URL(url_string);
        var code = url.searchParams.get("code");

        console.log(code);

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

        var body = "grant_type=authorization_code&redirect_uri=http%3A%2F%2Flocalhost%3A1337%2Fapi%2Fl3v3l%2Fcallback&code="+code;

        axios.post(`https://connect.playtix.team/oauth2/aus7e5j3kfGHKetdl5d7/v1/token`, body, config)
            .then(data => {
                console.log(data)

                // return data;
                // ctx.body = data;
            })
            .catch(error => {
                console.error(error);
                // return {err : "error"}
            });
  

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
   